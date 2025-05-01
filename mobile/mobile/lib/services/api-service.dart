import 'dart:io';

import 'package:dio/dio.dart';
import 'package:dio/io.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:mobile/services/interceptors/cookie-interceptor.dart';

class ApiService {
  final Dio _dio;

  ApiService({Dio? dio}) : _dio = _createDioClient();

  Future<Response> get(String path, {Map<String, dynamic>? queryParameters}) {
    return _dio.get(path, queryParameters: queryParameters);
  }

  Future<Response> post(String path, {dynamic data}) {
    return _dio.post(path, data: data);
  }

  Future<Response> patch(String path, {dynamic data}) {
    return _dio.patch(path, data: data);
  }

  Future<Response> put(String path, {dynamic data}) {
    return _dio.put(path, data: data);
  }

  Future<Response> delete(String path, {dynamic data}) {
    return _dio.delete(path, data: data);
  }

  static Dio _createDioClient() {
    final Dio dio = Dio(
      BaseOptions(
        baseUrl: dotenv.env['API_DOMAIN'] ?? '',
        connectTimeout: Duration(minutes: 5),
        receiveTimeout: Duration(minutes: 3),
        headers: {
          'Content-Type': 'application/json',
        },
      ),
    );

    dio.interceptors.add(CookieInterceptor());

    if (dotenv.get('ENVIRONMENT') == 'development') {
      (dio.httpClientAdapter as DefaultHttpClientAdapter).onHttpClientCreate =
          (HttpClient client) {
        client.badCertificateCallback =
            (X509Certificate cert, String host, int port) => true;
        return client;
      };

      dio.interceptors.add(LogInterceptor(
        request: true,
        requestBody: true,
        responseBody: true,
        responseHeader: false,
      ));
    }

    return dio;
  }
}