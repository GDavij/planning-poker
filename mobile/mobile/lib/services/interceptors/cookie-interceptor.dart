import 'package:dio/dio.dart';

class CookieInterceptor extends Interceptor {
  static String? _cookie;

  @override
  void onResponse(Response response, ResponseInterceptorHandler handler) {
    // Capture the Set-Cookie header from the response
    if (response.headers['Set-Cookie'] != null) {
      _cookie = response.headers['Set-Cookie']!.join('; ');
    }
    super.onResponse(response, handler);
  }

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    // Add the Cookie header to the request if available
    if (_cookie != null) {
      options.headers['Cookie'] = _cookie;
    }
    super.onRequest(options, handler);
  }
}