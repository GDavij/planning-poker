import 'package:flutter/material.dart';
import 'package:mobile/pages/sign-in-page.dart';

class UnauthenticatedLayout extends StatelessWidget {
  const UnauthenticatedLayout({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Navigator(
        initialRoute: '/sign-in',
        onGenerateRoute: (settings) {
          switch (settings.name) {
            case '/sign-in':
              return MaterialPageRoute(
                builder: (context) => const SignInPage(),
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
  }
}