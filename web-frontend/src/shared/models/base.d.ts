export interface Notification {
  message: string;
  code: string;
  httpStatusCode: number;
}

export interface MetaData {
  traceId: string;
  operationId: string;
  parentId: string;
  timestamp: string | Date;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T | null;
  notifications?: Notification[];
  meta: MetaData;
}
