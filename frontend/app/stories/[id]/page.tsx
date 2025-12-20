'use client';

import StoryViewer from '@/components/story/StoryViewer';
import AuthGuard from '@/components/auth/AuthGuard';

interface StoryPageProps {
  params: {
    id: string;
  };
}

export default function StoryPage({ params }: StoryPageProps) {
  return (
    <AuthGuard>
      <div className="min-h-screen bg-gray-100">
        <StoryViewer storyId={params.id} />
      </div>
    </AuthGuard>
  );
}

