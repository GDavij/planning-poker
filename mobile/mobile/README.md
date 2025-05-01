### Welcome to the Planning Poker App!

Thank you for your interest in the Planning Poker app! This guide will help you set up, build, and deploy the app with ease. If you encounter any issues, feel free to reach out or consult the official documentation for the tools mentioned.

### Tech Overview
Here are the key technologies used in this project:

1. **Flutter**  
   - **Purpose**: A UI toolkit for building natively compiled applications for mobile, web, and desktop from a single codebase.  
   - **Usage**: The app's UI and navigation are built using Flutter widgets like `Scaffold`, `AppBar`, and `Navigator`.

2. **Dart**  
   - **Purpose**: A programming language optimized for building fast apps on any platform.  
   - **Usage**: The primary language for writing the app's logic, UI, and state management.

3. **Firebase**  
   - **Purpose**: A platform for backend services like authentication, database, and analytics.  
   - **Usage**: Used for user authentication and other backend services (e.g., `AuthService`).

4. **Kotlin/Java**  
   - **Purpose**: Native languages for Android development.  
   - **Usage**: Required for integrating Flutter with Android-specific features or plugins.

5. **Gradle**  
   - **Purpose**: A build automation tool for managing dependencies and building Android apps.  
   - **Usage**: Used to configure and build the Android part of the Flutter app.

6. **C++**  
   - **Purpose**: A general-purpose programming language often used for performance-critical components.  
   - **Usage**: May be used for native libraries or plugins in the Android project.

7. **Pub**  
   - **Purpose**: Dart's package manager.  
   - **Usage**: Manages dependencies listed in `pubspec.yaml` (e.g., `go_router`, `flutter`, etc.).

These technologies work together to create a robust cross-platform mobile app with a focus on Android.

---

### Prerequisites
Before you begin, please ensure you have the following tools installed and configured:

1. **Flutter SDK**: Install the latest version of Flutter from [flutter.dev](https://flutter.dev/docs/get-started/install).  
2. **Dart SDK**: Comes bundled with Flutter.  
3. **Android Studio**: Install Android Studio for Android development.  
   - Make sure to install the **Flutter** and **Dart** plugins.  
4. **Xcode** (for iOS development): Install Xcode on macOS.  
5. **Firebase Setup**: Configure Firebase for authentication and other services.  
6. **Dependencies**: Ensure all required dependencies are listed in `pubspec.yaml`.

---

### Steps to Build the App

1. **Clone the Repository**  
   Start by cloning the repository to your local machine:  
   ```bash
   git clone https://github.com/GDavij/your-repo-name.git
   cd your-repo-name
   ```

2. **Install Dependencies**  
   Run the following command to install all required Dart and Flutter packages:  
   ```bash
   flutter pub get
   ```

3. **Configure Firebase**  
   - Download the `google-services.json` file for Android and place it in the `android/app` directory.  
   - For iOS, download the `GoogleService-Info.plist` file and place it in the `ios/Runner` directory.  
   - Ensure Firebase is properly initialized in the app.

4. **Run the App**  
   To run the app on your device or emulator:  
   - For Android:  
     ```bash
     flutter run -d android
     ```  
   - For iOS:  
     ```bash
     flutter run -d ios
     ```

5. **Build the APK (Android)**  
   To generate a release APK:  
   ```bash
   flutter build apk --release
   ```

6. **Build the App Bundle (Android)**  
   For publishing to the Google Play Store:  
   ```bash
   flutter build appbundle --release
   ```

7. **Build for iOS**  
   Ensure you have a valid Apple Developer account and provisioning profile:  
   ```bash
   flutter build ios --release
   ```

8. **Testing**  
   Run tests to ensure the app works as expected:  
   ```bash
   flutter test
   ```

9. **Deployment**  
   - **Android**: Upload the generated `.aab` file to the Google Play Console.  
   - **iOS**: Use Xcode to archive and upload the app to the App Store.

---

### Notes
- Please ensure all environment variables (e.g., Firebase keys) are properly configured.  
- Update the `pubspec.yaml` file if new dependencies are added.  
- Use `flutter doctor` to verify your environment setup.  

We hope you enjoy working with the Planning Poker app. Happy coding!