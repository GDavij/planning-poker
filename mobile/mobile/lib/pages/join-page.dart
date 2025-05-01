import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:go_router/go_router.dart';
import 'package:mobile/drivers/signalr-driver.dart';

class JoinMatchPage extends StatefulWidget {
  final int matchId;

  const JoinMatchPage({Key? key, required this.matchId}) : super(key: key);

  @override
  State<JoinMatchPage> createState() => _JoinMatchPageState();
}

class _JoinMatchPageState extends State<JoinMatchPage> {
  final SignalRDriver _signalRDriver = SignalRDriver();

  @override
  void initState() {
    super.initState();
    _signalRDriver.Reconnect();
    _registerAndJoinMatch();
  }

  Future<void> _registerAndJoinMatch() async {
    try {
      await _signalRDriver.registerEndpoint('ApproveJoinRequest', (arguments) {
        context.go('/dashboard/matches/party/${widget.matchId}');
      });

      await _signalRDriver.invokeMethod('JoinMatchAsync', widget.matchId);
    } catch (e) {
      debugPrint('Error: $e');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Container(
          alignment: Alignment.center,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const CircularProgressIndicator(),
                  const SizedBox(width: 16),
                  const Text(
                    'Joining...',
                    style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
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