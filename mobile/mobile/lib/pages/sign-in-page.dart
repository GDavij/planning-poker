import 'package:flutter/material.dart';
import 'package:mobile/forms/auth-form.dart';

class SignInPage extends StatelessWidget {
  const SignInPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SignInForm(),
    );
  }
}