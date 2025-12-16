import Link from 'next/link';

// SSGでストーリー一覧を生成
export default function StoriesPage() {
  // 実際の実装では、APIからストーリー一覧を取得
  const stories = [
    {
      id: 'story-1',
      title: '企業セキュリティの脅威',
      description: '企業ネットワークに侵入の兆候を発見します。',
    },
  ];

  return (
    <div className="min-h-screen bg-gray-100">
      <div className="max-w-6xl mx-auto p-8">
        <h1 className="text-4xl font-bold mb-8">ストーリー一覧</h1>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {stories.map((story) => (
            <Link
              key={story.id}
              href={`/stories/${story.id}`}
              className="bg-white rounded-lg shadow-lg p-6 hover:shadow-xl transition-shadow"
            >
              <h2 className="text-2xl font-semibold mb-2">{story.title}</h2>
              {story.description && (
                <p className="text-gray-600">{story.description}</p>
              )}
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}

