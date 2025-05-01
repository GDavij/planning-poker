import 'dart:convert';
import 'dart:io';

import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:signalr_netcore/http_connection_options.dart';
import 'package:signalr_netcore/hub_connection.dart';
import 'package:signalr_netcore/hub_connection_builder.dart';
import 'package:signalr_netcore/ihub_protocol.dart';
import 'package:signalr_netcore/itransport.dart';
import 'package:signalr_netcore/json_hub_protocol.dart';
import 'package:signalr_netcore/signalr_http_client.dart';

class SignalRDriver {
  static SignalRDriver _instance = SignalRDriver._internal();

  late HubConnection _hubConnection;

  // Private constructor
  SignalRDriver._internal() {
    _initializeSignalR("${dotenv.get("SIGNALR_HUB_DOMAIN")}/matchHub");
  }

  // Public factory to access the singleton instance
  factory SignalRDriver() {
      if (_instance._hubConnection.state != HubConnectionState.Connected) {
        _instance = SignalRDriver._internal();
      }
    return _instance;
  }

  Future<void> _initializeSignalR(String url) async {
    final httpClient = CustomSignalRHttpClient();

    _hubConnection = HubConnectionBuilder()
        .withUrl(
      url,
      options: HttpConnectionOptions(
        httpClient: httpClient,
        transport: HttpTransportType.WebSockets,
        accessTokenFactory: () async {
          final user = FirebaseAuth.instance.currentUser;
          if (user != null) {
            return (await user.getIdToken())!;
          }
          throw Exception('User not authenticated');
        },
      ),
    )
        .withHubProtocol(JsonHubProtocol())
        .withAutomaticReconnect(retryDelays: [2000, 5000, 10000])
        .build();

    try {
      while (_hubConnection.state != HubConnectionState.Connected) {
        await Reconnect();
        debugPrint('Waiting for connection to be in the Connected state..., ${_hubConnection.state?.name}');
        await Future.delayed(const Duration(milliseconds: 500));
      }
    } catch (e) {
      debugPrint('Error starting SignalR connection: $e');
    }
  }

  Future<void> Reconnect() async {
    try {
      await _hubConnection.start();
      debugPrint('SignalR connection started');

    } catch (e) {
      debugPrint('Error starting SignalR connection: $e');
    }
  }

  Future<void> invokeMethod(String methodName, dynamic args) async {
    try {
      while (_hubConnection.state != HubConnectionState.Connected) {
        await Reconnect();
        debugPrint('Waiting for connection to be in the Connected state..., ${_hubConnection.state?.name}');
        await Future.delayed(const Duration(milliseconds: 500));
      }
      await _hubConnection.invoke(methodName, args: [args]);
    } catch (e) {
      debugPrint('Error invoking method $methodName: $e');
    }
  }

  Future<void> registerEndpoint(String methodName, Function callback) async {
    while (_hubConnection.state != HubConnectionState.Connected) {
      await Reconnect();
      debugPrint('Waiting for connection to be in the Connected state..., ${_hubConnection.state?.name}');
      await Future.delayed(const Duration(milliseconds: 500));
    }
    _hubConnection.on(methodName, (arguments) {
      callback(arguments);
    });
  }
}

class CustomSignalRHttpClient extends SignalRHttpClient {
  final String? authToken;

  CustomSignalRHttpClient({this.authToken});

  @override
  Future<SignalRHttpResponse> send(SignalRHttpRequest request) async {
    final httpClient = HttpClient();

    if (dotenv.get("ENVIRONMENT") == "development") {
      httpClient.badCertificateCallback =
          (X509Certificate cert, String host, int port) => true;
    }

    final httpRequest = await httpClient.openUrl(
      request.method ?? "",
      Uri.parse(request.url ?? ""),
    );
    httpRequest.headers.set('Content-Type', 'application/json');

    // Add headers
    if (request.headers != null) {
      for (var header in request.headers!.names) {
        httpRequest.headers.set(
          header,
          request.headers!.getHeaderValue(header)!,
        );
      }
    }

    // Add Authorization header or cookie
    if (authToken != null) {
      httpRequest.headers.set('Authorization', 'Bearer $authToken');
      httpRequest.cookies.add(Cookie('Authorization', authToken!));
    }

    if (request.content != null) {
      if (request.content != null) {
        httpRequest.write(request.content);
      }
    }

    final httpResponse = await httpRequest.close();
    final responseBody = await httpResponse.fold<List<int>>(
      [],
      (previous, element) => previous..addAll(element),
    );

    final responseContent = utf8.decode(responseBody);
    debugPrint('Negotiation Response: $responseContent');

    return SignalRHttpResponse(
      httpResponse.statusCode,
      statusText: httpResponse.reasonPhrase,
      content: responseContent,
    );
  }

  @override
  Future<void> close() async {
    // No additional cleanup needed
  }
}
