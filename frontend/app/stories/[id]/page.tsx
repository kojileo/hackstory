import StoryViewer from '@/components/story/StoryViewer';

interface StoryPageProps {
  params: {
    id: string;
  };
}

export default function StoryPage({ params }: StoryPageProps) {
  return (
    <div className="min-h-screen bg-gray-100">
      <StoryViewer storyId={params.id} />
    </div>
  );
}

