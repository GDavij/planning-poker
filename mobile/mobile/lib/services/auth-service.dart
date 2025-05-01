import 'package:mobile/models/auth-models.dart';
import 'package:mobile/services/api-service.dart';

class AuthService {
  final ApiService _api = ApiService();

  Future<void> saveSession(String token) async {
    await _api.post(
      '/auth/save-session',
      data: {'oAuthToken': token},
    );
  }

  Future<void> autoLogin() async {
    await _api.post('/auth/autologin');
  }

  Future<Me> getCurrentAccount() async {
    final response = await _api.get('/auth/me');
    return Me.fromJson(response.data);
  }
}