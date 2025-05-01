import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:mobile/pages/create-match-page.dart';
import 'package:mobile/pages/dashboard-page.dart';
import 'package:mobile/pages/join-page.dart';
import 'package:mobile/pages/party-page.dart';
import 'package:mobile/pages/sign-in-page.dart';
import 'package:firebase_core/firebase_core.dart';
import 'drivers/signalr-driver.dart';
import 'layouts/authenticated-layout.dart';
import 'layouts/unauthenticated-layout.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

final ThemeData appTheme = ThemeData(
  primaryColor: const Color(0xFFAAAAFF), // Primary main color
  primaryColorLight: const Color(0xFFCCCCFF), // Primary light color
  primaryColorDark: const Color(0xFF8888DD), // Primary dark color
  colorScheme: ColorScheme(
    primary: const Color(0xFFAAAAFF),
    primaryContainer: const Color(0xFFCCCCFF),
    secondary: const Color(0xFFDDEEFF),
    secondaryContainer: const Color(0xFFEEEEFF),
    surface: const Color(0xFFFFFFFF),
    background: const Color(0xFFFFFFFF),
    error: Colors.red,
    onPrimary: const Color(0xFFFFFFFF), // Contrast text for primary
    onSecondary: const Color(0xFF333333), // Contrast text for secondary
    onSurface: const Color(0xFF333333),
    onBackground: const Color(0xFF333333),
    onError: Colors.white,
    brightness: Brightness.light,
  ),
  scaffoldBackgroundColor: const Color(0xFFFFFFFF), // Background default
  cardColor: const Color(0xFFFAFAFA), // Paper background
  textTheme: const TextTheme(
    bodyLarge: TextStyle(
      color: Color(0xFF333333), // Primary text
      fontFamily: 'Roboto',
    ),
    bodyMedium: TextStyle(
      color: Color(0xFF555555), // Secondary text
      fontFamily: 'Roboto',
    ),
    bodySmall: TextStyle(
      fontWeight: FontWeight.w700, // Button font weight
      fontFamily: 'Roboto',
    ),
  ),
  elevatedButtonTheme: ElevatedButtonThemeData(
    style: ElevatedButton.styleFrom(
      padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(6), // Button border radius
        side: const BorderSide(color: Color(0xFFEEEEFF)), // Button border
      ),
      elevation: 0, // Disable shadow
    ).copyWith(
      overlayColor: MaterialStateProperty.all(const Color(0xFFCCCCFF)), // Hover color
    ),
  ),
);

Future main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Load the .env file
  await dotenv.load();

  // Initialize SignalRDriver
  SignalRDriver();

  // Initialize Firebase
  await Firebase.initializeApp(
    options: FirebaseOptions(
      apiKey: dotenv.get("FIREBASE_API_KEY"),
      appId: dotenv.get("FIREBASE_AUTH_DOMAIN"),
      messagingSenderId: "",
      projectId: dotenv.get("FIREBASE_PROJECT_ID"),
    ),
  );

  runApp(MaterialApp(
      theme: appTheme,
      home: MyApp()
  ));


}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final router = GoRouter(
      redirect: (context, state) {
        final isAuthenticated = FirebaseAuth.instance.currentUser != null;

        // Redirect unauthenticated users to the login page
        if (!isAuthenticated && state.matchedLocation != '/') {
          return '/';
        }
        return null; // No redirection
      },
      routes: [
        GoRoute(
          path: '/',
          builder: (context, state) => const UnauthenticatedLayout(),
          routes: [
            GoRoute(
              path: 'sign-in',
              builder: (context, state) => const SignInPage(),
            ),
          ],
        ),
        GoRoute(
          path: '/dashboard',
          builder: (context, state) =>
             AuthenticatedLayout(),

          routes: [
            GoRoute(
              path: 'home',
              builder: (context, state) =>
               HomeDashboard(),

            ),
            GoRoute(
              path: 'matches/new',
              builder: (context, state) =>
              const CreateMatchPage(),
            ),
            GoRoute(
              path: 'matches/join/:matchId',
              builder: (context, state) {
                final matchId = state.pathParameters['matchId']!;
                return JoinMatchPage(matchId: int.parse(matchId));
              },
            ),
            GoRoute(
              path: 'matches/party/:matchId',
              builder: (context, state) {
                final matchId = state.pathParameters['matchId']!;
                return PartyPage(matchId: int.parse(matchId)
                );
              },
            ),
          ],
        ),
      ],
    );

    FirebaseAuth.instance.authStateChanges().listen((User? user) {
      router.refresh();
    });

    return MaterialApp.router(
      routerConfig: router,
    );
  }
}
