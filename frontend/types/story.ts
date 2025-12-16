export interface Story {
  id: string;
  title: string;
  description?: string;
  chapters: Chapter[];
}

export interface Chapter {
  id: number;
  title: string;
  content: string;
  choices: Choice[];
}

export interface Choice {
  id: string;
  text: string;
  nextChapter: number;
  effects?: {
    knowledge?: number;
  };
}

export interface ChapterProgress {
  storyId: string;
  chapterId: number;
  completed: boolean;
  choices?: Record<string, unknown>;
}

