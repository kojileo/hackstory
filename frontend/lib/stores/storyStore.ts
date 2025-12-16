import { create } from 'zustand';
import type { Story, Chapter, ChapterProgress } from '@/types/story';

interface StoryState {
  currentStory: Story | null;
  currentChapter: Chapter | null;
  progress: Map<string, ChapterProgress>;
  isLoading: boolean;
  error: string | null;
  
  // Actions
  loadStory: (storyId: string) => Promise<void>;
  setCurrentChapter: (chapterId: number) => void;
  saveProgress: (storyId: string, chapterId: number, completed: boolean, choices?: Record<string, unknown>) => Promise<void>;
  reset: () => void;
}

export const useStoryStore = create<StoryState>((set, get) => ({
  currentStory: null,
  currentChapter: null,
  progress: new Map(),
  isLoading: false,
  error: null,

  loadStory: async (storyId: string) => {
    set({ isLoading: true, error: null });
    
    try {
      // 静的JSONファイルから読み込み（API呼び出しなし）
      const response = await fetch(`/stories/${storyId}.json`);
      if (!response.ok) {
        throw new Error('Story not found');
      }
      
      const story: Story = await response.json();
      set({ 
        currentStory: story, 
        currentChapter: story.chapters[0] || null,
        isLoading: false 
      });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : 'Failed to load story',
        isLoading: false 
      });
    }
  },

  setCurrentChapter: (chapterId: number) => {
    const { currentStory } = get();
    if (!currentStory) return;
    
    const chapter = currentStory.chapters.find(c => c.id === chapterId);
    if (chapter) {
      set({ currentChapter: chapter });
    }
  },

  saveProgress: async (storyId: string, chapterId: number, completed: boolean, choices?: Record<string, unknown>) => {
    try {
      const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
      // デバウンス処理は呼び出し側で実装
      const response = await fetch(`${apiUrl}/api/progress/stories/${storyId}/chapters/${chapterId}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          completed,
          choices,
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to save progress');
      }

      const progress: ChapterProgress = {
        storyId,
        chapterId,
        completed,
        choices,
      };

      set((state) => {
        const newProgress = new Map(state.progress);
        newProgress.set(`${storyId}-${chapterId}`, progress);
        return { progress: newProgress };
      });
    } catch (error) {
      console.error('Error saving progress:', error);
    }
  },

  reset: () => {
    set({
      currentStory: null,
      currentChapter: null,
      progress: new Map(),
      error: null,
    });
  },
}));

