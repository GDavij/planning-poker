import { ApiResponse } from "../models/base";
import { Notification } from "../models/notification";

export class integrationError extends Error {
  constructor(apiResponse: ApiResponse<unknown>) {
    const errorMessage = apiResponse
      .notifications!.map((m) => m.message)
      .join(`\n`);
    super(errorMessage);
  }
}
