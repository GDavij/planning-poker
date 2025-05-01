import 'package:flutter/material.dart';
import 'package:mobile/drivers/signalr-driver.dart';
import 'package:mobile/models/matches-models.dart';
import 'package:mobile/services/match-service.dart';

import '../forms/edit-story-modal-form.dart';

class PartyPage extends StatefulWidget {
  final int matchId;

  const PartyPage({Key? key, required this.matchId}) : super(key: key);

  @override
  State<PartyPage> createState() => _PartyPageState();
}

class _PartyPageState extends State<PartyPage> {
  final SignalRDriver _signalRDriver = SignalRDriver();
  final MatchService _matchService = MatchService();

  List<Story> stories = [];
  Story? currentShowingStory;

  @override
  void initState() {
    super.initState();
    _signalRDriver.Reconnect();
    _fetchStories();
    _initializeSignalR();
  }

  Future<void> _initializeSignalR() async {
    try {
      await _signalRDriver.registerEndpoint('UpdateStoriesOfMatchWith', (arguments) {
        var serverStories = arguments[0] as List<dynamic>;
        setState(() {
          stories = serverStories.map((item) => Story.fromJson(item as Map<String, dynamic>)).toList();
        });
      });

      await _signalRDriver.registerEndpoint("SelectStoryToVoteAs", (arguments) {
        int storyId = arguments[0] as int;
        debugPrint("SelectStoryToVoteAs: $storyId");

        if (mounted) {

          Story? selectedStory = stories.where((story) => story.storyId == storyId).firstOrNull;

          setState(() {
            debugPrint("Selected Story is find as: $currentShowingStory for $stories");
            if (selectedStory != null) {
              currentShowingStory = selectedStory;
            }
          });
        }
      });

      await _signalRDriver.registerEndpoint("ParticipantVoteForStoryIs", (arguments) {
        _showComplexityModal(context);
      });

      await _signalRDriver.invokeMethod('JoinMatchAsync', widget.matchId);
    } catch (e) {
      debugPrint('Error initializing SignalR: $e');
    }
  }

  Future<void> _fetchStories() async {
    stories = await _matchService.listMatchStories(widget.matchId);
    debugPrint("Stories: $stories");
  }

  void handleSelectStory(Story story) {
    setState(() {
      currentShowingStory = story;
    });
  }

  void _showComplexityModal(BuildContext context) {
    final complexities = [
      {'points': 1, 'description': "Low complexity"},
      {'points': 2, 'description': "A little complex"},
      {'points': 3, 'description': "Basic complexity"},
      {'points': 5, 'description': "Maybe takes me a Day"},
      {'points': 8, 'description': "Can take more than a Day"},
      {'points': 13, 'description': "This surely is complex"},
      {'points': 21, 'description': "This gonna be good"},
    ];

    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (BuildContext context) {
        return Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Text(
                'Select Complexity',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 16),
              Expanded(
                child: ListView.builder(
                  shrinkWrap: true,
                  itemCount: complexities.length,
                  itemBuilder: (context, index) {
                    final complexity = complexities[index];
                    return Card(
                      margin: const EdgeInsets.symmetric(vertical: 8),
                      child: ListTile(
                        title: Text(complexity['description'].toString()),
                        subtitle: Text('Points: ${complexity['points']}'),
                        onTap: () async {
                          Navigator.pop(context);
                          if (currentShowingStory != null) {
                            try {
                              await _matchService.voteForStory(
                                currentShowingStory!.matchId,
                                currentShowingStory!.storyId,
                                complexity['points'] as int,
                              );
                              ScaffoldMessenger.of(context).showSnackBar(
                                SnackBar(
                                  content: Text(
                                    'Vote submitted successfully: ${complexity['description']} (${complexity['points']} points)',
                                  ),
                                  backgroundColor: Colors.green,
                                ),
                              );
                            } catch (e) {
                              ScaffoldMessenger.of(context).showSnackBar(
                                SnackBar(
                                  content: Text(
                                    'Failed to submit vote: ${complexity['description']} (${complexity['points']} points)',
                                  ),
                                  backgroundColor: Colors.red,
                                ),
                              );
                            }
                          }
                        },
                      ),
                    );
                  },
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Party Page'),
        backgroundColor: Colors.blueAccent,
      ),
      body: Container(
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            colors: [Colors.blueAccent, Colors.lightBlue],
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
          ),
        ),
        child: Column(
          children: [
            // Main Content Section
            Expanded(
              child: currentShowingStory == null
                  ? Center(
                child: Text(
                  'No Story Selected To Analyze Yet...',
                  style: TextStyle(
                    fontSize: 18,
                    color: Colors.white70,
                  ),
                ),
              )
                  : Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  CurrentlyShowingStoryViewer(
                    story: currentShowingStory!,
                  ),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: () {
                      _showComplexityModal(context);
                    },
                    child: const Text('Vote for this Story'),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class CurrentlyShowingStoryViewer extends StatelessWidget {
  final Story story;

  const CurrentlyShowingStoryViewer({Key? key, required this.story})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Card(
        elevation: 4,
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Story Details',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 16),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Name',
                        style: TextStyle(color: Colors.grey),
                      ),
                      Text(
                        story.name ?? 'N/A',
                        style: TextStyle(
                            fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                    ],
                  ),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Story Number',
                        style: TextStyle(color: Colors.grey),
                      ),
                      Text(
                        story.storyId.toString() ?? 'N/A',
                        style: TextStyle(
                            fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                    ],
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}