export interface Challenge {
  id: string;
  title: string;
  description?: string;
  difficulty: number; // 1-5
  solution: string;
  hints?: string[];
}

export interface ChallengeProgress {
  challengeId: string;
  completed: boolean;
  attempts: number;
  completedAt?: string;
}

