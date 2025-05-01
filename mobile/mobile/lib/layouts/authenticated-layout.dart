import 'package:flutter/material.dart';
import 'package:mobile/pages/create-match-page.dart';
import 'package:mobile/pages/dashboard-page.dart';
import 'package:mobile/pages/join-page.dart';
import 'package:mobile/pages/party-page.dart';
import 'package:mobile/services/auth-service.dart';

class AuthenticatedLayout extends StatefulWidget {
  const AuthenticatedLayout({Key? key}) : super(key: key);

  @override
  State<AuthenticatedLayout> createState() => _AuthenticatedLayoutState();
}

class _AuthenticatedLayoutState extends State<AuthenticatedLayout> {
  final AuthService _authService = AuthService();
  late Future<void> _authCheckFuture;

  @override
  void initState() {
    super.initState();
    _authCheckFuture = _checkAuthentication();
  }

  Future<void> _checkAuthentication() async {
    try {
      final account = await _authService.getCurrentAccount();
      // Set account ID in your state management solution
    } catch (e) {
      Navigator.pushReplacementNamed(context, '/sign-in');
    }
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<void>(
      future: _authCheckFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }

        if (snapshot.hasError) {
          return const Center(child: Text('Error loading account'));
        }

        return Scaffold(
          appBar: AppBar(
            title: null, // Removed the "Authenticated Layout" text
          ),
          body: Navigator(
            initialRoute: '/dashboard/home',
            onGenerateRoute: (settings) {
              switch (settings.name) {
                case '/dashboard/home':
                  return MaterialPageRoute(
                    builder: (context) => const HomeDashboard(),
                  );
                case '/dashboard/matches/new':
                  return MaterialPageRoute(
                    builder: (context) => const CreateMatchPage(),
                  );
                case '/dashboard/matches/join/:matchId':
                  final matchId = settings.arguments as int;
                  return MaterialPageRoute(
                    builder: (context) => JoinMatchPage(matchId: matchId),
                  );
                case '/dashboard/matches/party/:matchId':
                  final matchId = settings.arguments as int;
                  return MaterialPageRoute(
                    builder: (context) => PartyPage(matchId: matchId),
                  );
                default:
                  return MaterialPageRoute(
                    builder: (context) => const Center(
                      child: Text('Page not found'),
                    ),
                  );
              }
            },
          ),
        );
      },
    );
  }
}