export interface Health {
  botHealthData: {
    pingTime: string;
    receivedMessages: number;
    deployTime: string;
    version: string;
  }
  refresherHealthData: {
    pingTime: string;
    deployTime: string;
    nextRefreshDate: string;
    version: string;
  }
  reminderHealthData: {
    pingTime: string;
    deployTime: string;
    nextNotifyDate: string;
    version: string;
  }
}
