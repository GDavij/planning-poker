import 'package:mobile/models/matches-models.dart';
import 'package:mobile/services/api-service.dart';

class MatchService {
  final ApiService _api = ApiService();

  Future<List<ListMatchesQueryResponse>> listUserCreatedMatches(
      int page, int limit) async {
    final response = await _api.get(
      '/matches',
      queryParameters: {'page': page, 'limit': limit},
    );
    return (response.data as List)
        .map((item) => ListMatchesQueryResponse.fromJson(item))
        .toList();
  }

  Future<StartMatchCommandResponse> startMatchAs(
      String description, int roleId, bool shouldSpectate) async {
    final response = await _api.post(
      '/matches/start',
      data: {
        'description': description,
        'roleId': roleId,
        'shouldObservate': shouldSpectate,
      },
    );
    return StartMatchCommandResponse.fromJson(response.data);
  }

  Future<List<ListRolesQueryResponse>> listMatchRoles() async {
    final response = await _api.get('/matches/roles');
    return (response.data as List)
        .map((item) => ListRolesQueryResponse.fromJson(item))
        .toList();
  }

  Future<List<Story>> listMatchStories(int matchId) async {
    final response = await _api.get('/matches/match/$matchId/stories');
    return (response.data as List)
        .map((item) => Story.fromJson(item))
        .toList();
  }

  Future<void> updateStory(Story story) async {
    await _api.put(
      '/matches/match/${story.matchId}/story/${story.storyId}/update',
      data: story.toJson(),
    );
  }

  Future<void> createStory(Story story) async {
    await _api.post(
      '/matches/match/${story.matchId}/story/add',
      data: story.toJson(),
    );
  }

  Future<void> deleteStory(Story story) async {
    await _api.delete(
      '/matches/match/${story.matchId}/story/${story.storyId}',
    );
  }

  Future<void> selectStoryToAnalyze(Story story) async {
    await _api.patch(
      '/matches/match/${story.matchId}/story/${story.storyId}',
    );
  }

  Future<List<Participant>> listParticipantsForMatch(int matchId) async {
    final response = await _api.get('/matches/match/$matchId/participants');
    return (response.data as List)
        .map((item) => Participant.fromJson(item))
        .toList();
  }

  Future<void> voteForStory(int matchId, int storyId, int points) async {
    await _api.patch(
      '/matches/match/$matchId/story/$storyId/vote/$points',
    );
  }

  Future<void> finishMatch(int matchId) async {
    await _api.patch('matches/match/$matchId/finish');
  }
}