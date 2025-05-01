class ListMatchesQueryResponse {
  final int matchId;
  final String description;
  final bool hasStarted;
  final bool hasClosed;

  ListMatchesQueryResponse({
    required this.matchId,
    required this.description,
    required this.hasStarted,
    required this.hasClosed,
  });

  factory ListMatchesQueryResponse.fromJson(Map<String, dynamic> json) {
    return ListMatchesQueryResponse(
      matchId: json['matchId'],
      description: json['description'],
      hasStarted: json['hasStarted'],
      hasClosed: json['hasClosed'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'matchId': matchId,
      'description': description,
      'hasStarted': hasStarted,
      'hasClosed': hasClosed,
    };
  }
}

class ListRolesQueryResponse {
  final int roleId;
  final String name;
  final String abbreviation;

  ListRolesQueryResponse({
    required this.roleId,
    required this.name,
    required this.abbreviation,
  });

  factory ListRolesQueryResponse.fromJson(Map<String, dynamic> json) {
    return ListRolesQueryResponse(
      roleId: json['roleId'],
      name: json['name'],
      abbreviation: json['abbreviation'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'roleId': roleId,
      'name': name,
      'abbreviation': abbreviation,
    };
  }
}

class StartMatchCommandResponse {
  final int matchId;

  StartMatchCommandResponse({required this.matchId});

  factory StartMatchCommandResponse.fromJson(Map<String, dynamic> json) {
    return StartMatchCommandResponse(
      matchId: json['matchId'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'matchId': matchId,
    };
  }
}

class StoryPointResponse {
  final int points;
  final String participantName;

  StoryPointResponse({
    required this.points,
    required this.participantName,
  });

  factory StoryPointResponse.fromJson(Map<String, dynamic> json) {
    return StoryPointResponse(
      points: json['points'],
      participantName: json['participantName'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'points': points,
      'participantName': participantName,
    };
  }
}

class Story {
  final int storyId;
  final String name;
  final int matchId;

  Story({required this.storyId, required this.name, required this.matchId});

  factory Story.fromJson(Map<String, dynamic> json) {
    return Story(
      storyId: json['storyId'] as int,
      name: json['name'] as String,
      matchId: json['matchId'] as int,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'storyId': storyId,
      'name': name,
      'matchId': matchId,
    };
  }
}

class Vote {
  final int storyId;
  final bool hasVotedAlready;
  final int points;

  Vote({
    required this.storyId,
    required this.hasVotedAlready,
    required this.points,
  });

  factory Vote.fromJson(Map<String, dynamic> json) {
    return Vote(
      storyId: json['storyId'],
      hasVotedAlready: json['hasVotedAlready'],
      points: json['points'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'storyId': storyId,
      'hasVotedAlready': hasVotedAlready,
      'points': points,
    };
  }
}

class Participant {
  final int accountId;
  final String roleName;
  final bool isSpectating;
  final String participantName;
  final List<Vote> votes;

  Participant({
    required this.accountId,
    required this.roleName,
    required this.isSpectating,
    required this.participantName,
    required this.votes,
  });

  factory Participant.fromJson(Map<String, dynamic> json) {
    return Participant(
      accountId: json['accountId'],
      roleName: json['roleName'],
      isSpectating: json['isSpectating'],
      participantName: json['participantName'],
      votes: (json['votes'] as List).map((item) => Vote.fromJson(item)).toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'accountId': accountId,
      'roleName': roleName,
      'isSpectating': isSpectating,
      'participantName': participantName,
      'votes': votes.map((item) => item.toJson()).toList(),
    };
  }
}