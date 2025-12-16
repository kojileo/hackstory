'use client';

import { useEffect, useCallback } from 'react';
import { useStoryStore } from '@/lib/stores/storyStore';
import { debounce } from '@/lib/utils/debounce';
import type { Choice } from '@/types/story';

interface StoryViewerProps {
  storyId: string;
}

export default function StoryViewer({ storyId }: StoryViewerProps) {
  const {
    currentStory,
    currentChapter,
    isLoading,
    error,
    loadStory,
    setCurrentChapter,
    saveProgress,
  } = useStoryStore();

  useEffect(() => {
    loadStory(storyId);
  }, [storyId, loadStory]);

  // デバウンス処理（500ms）
  const debouncedSaveProgress = useCallback(
    debounce((chapterId: number, completed: boolean, choices?: Record<string, unknown>) => {
      saveProgress(storyId, chapterId, completed, choices);
    }, 500),
    [storyId, saveProgress]
  );

  const handleChoiceClick = (choice: Choice) => {
    if (!currentChapter || !currentStory) return;

    // 進捗を保存
    debouncedSaveProgress(currentChapter.id, true, {
      choiceId: choice.id,
      nextChapter: choice.nextChapter,
    });

    // 次のチャプターに移動
    setCurrentChapter(choice.nextChapter);
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-lg">読み込み中...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-red-500">エラー: {error}</div>
      </div>
    );
  }

  if (!currentStory || !currentChapter) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-lg">ストーリーが見つかりません</div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold mb-2">{currentStory.title}</h1>
        {currentStory.description && (
          <p className="text-gray-600">{currentStory.description}</p>
        )}
      </div>

      <div className="bg-white rounded-lg shadow-lg p-8 mb-6">
        <h2 className="text-2xl font-semibold mb-4">{currentChapter.title}</h2>
        <div className="prose max-w-none mb-6">
          <p className="whitespace-pre-line text-gray-800">{currentChapter.content}</p>
        </div>

        {currentChapter.choices.length > 0 ? (
          <div className="space-y-3">
            {currentChapter.choices.map((choice) => (
              <button
                key={choice.id}
                onClick={() => handleChoiceClick(choice)}
                className="w-full text-left p-4 border-2 border-gray-300 rounded-lg hover:border-blue-500 hover:bg-blue-50 transition-colors"
              >
                {choice.text}
              </button>
            ))}
          </div>
        ) : (
          <div className="text-center py-8">
            <p className="text-lg font-semibold text-green-600">チャプター完了！</p>
            <p className="text-gray-600 mt-2">おめでとうございます！</p>
          </div>
        )}
      </div>

      <div className="text-sm text-gray-500 text-center">
        チャプター {currentChapter.id} / {currentStory.chapters.length}
      </div>
    </div>
  );
}

