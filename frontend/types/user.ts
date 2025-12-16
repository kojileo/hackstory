export interface User {
  id: string;
  email: string;
  username?: string;
  createdAt: string;
}

export interface UserProgress {
  level: number;
  experience: number;
  totalChaptersCompleted: number;
  totalChallengesCompleted: number;
}

