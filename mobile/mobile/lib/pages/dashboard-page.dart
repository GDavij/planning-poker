import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:go_router/go_router.dart';
import 'package:mobile/models/matches-models.dart';
import 'package:mobile/services/match-service.dart';

class HomeDashboard extends StatefulWidget {
  const HomeDashboard({super.key});

  @override
  State<HomeDashboard> createState() => _HomeDashboardState();
}

class _HomeDashboardState extends State<HomeDashboard> {
  final MatchService _matchService = MatchService();
  final TextEditingController _matchIdController = TextEditingController();

  bool isLoadingCreatedMatches = false;
  List<ListMatchesQueryResponse> userCreatedMatches = [];

  @override
  void initState() {
    super.initState();
    loadTop10CreatedMatches();
  }

  Future<void> loadTop10CreatedMatches() async {
    setState(() {
      isLoadingCreatedMatches = true;
    });

    try {
      final matches = await _matchService.listUserCreatedMatches(1, 3);
      setState(() {
        userCreatedMatches = matches;
      });
    } catch (e) {
      // Handle error
    } finally {
      setState(() {
        isLoadingCreatedMatches = false;
      });
    }
  }

  void joinMatchById() {
    final matchId = int.tryParse(_matchIdController.text);
    if (matchId != null) {
      context.go('/dashboard/matches/join/$matchId');
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please enter a valid match ID')),
      );
    }
  }

  void joinMatch(ListMatchesQueryResponse match) {
    context.go('/dashboard/matches/join/${match.matchId}');
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Dashboard'),
        automaticallyImplyLeading: false, // Removes the back button
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Card(
          child: Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                const Text(
                  'Matches',
                  style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
                  textAlign: TextAlign.center,
                ),
                const SizedBox(height: 16),
                TextField(
                  controller: _matchIdController,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(
                    labelText: 'Enter Match ID',
                    border: OutlineInputBorder(),
                  ),
                ),
                const SizedBox(height: 8),
                ElevatedButton(
                  onPressed: joinMatchById,
                  child: const Text('Join Match'),
                ),
                const Divider(),
                const Text(
                  'Known Matches',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 8),
                Expanded(
                  child: isLoadingCreatedMatches
                      ? const Center(child: CircularProgressIndicator())
                      : userCreatedMatches.isEmpty
                      ? const Center(child: Text('No matches created yet.'))
                      : ListView.builder(
                    itemCount: userCreatedMatches.length,
                    itemBuilder: (context, index) {
                      final match = userCreatedMatches[index];
                      return ListTile(
                        title: Text(match.description),
                        subtitle: Text('Match ID: ${match.matchId}'),
                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            IconButton(
                              icon: const Icon(Icons.arrow_forward),
                              onPressed: () => joinMatch(match),
                            ),
                          ],
                        ),
                      );
                    },
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}