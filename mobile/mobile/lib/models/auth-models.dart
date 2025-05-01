class Me {
  final int accountId;

  Me({required this.accountId});

  factory Me.fromJson(Map<String, dynamic> json) {
    return Me(
      accountId: json['accountId'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'accountId': accountId,
    };
  }
}