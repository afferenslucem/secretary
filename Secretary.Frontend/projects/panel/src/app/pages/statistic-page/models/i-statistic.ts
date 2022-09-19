export interface Statistic {
  documentStatistic: {
    timeOffCount: number;
    vacationCount: number;
    distantCount: number;
  }
  userStatistic: {
    totalUsers: number;
    userWithDocuments: number;
  }
}
