export interface Health {
  botHealthData: {
    pingTime: string;
    deployTime: string;
    receivedMessages: number;
    version: string;
  }
  refresherHealthData: {
    pingTime: string;
    nextRefreshDate: string;
  }
  reminderHealthData: {
    pingTime: string;
    nextNotifyDate: string;
  }
}
