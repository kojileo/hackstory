import Link from 'next/link';

export default function Home() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 to-gray-800">
      <div className="max-w-6xl mx-auto px-8 py-16">
        <div className="text-center mb-12">
          <h1 className="text-5xl font-bold text-white mb-4">
            HackStory
          </h1>
          <p className="text-xl text-gray-300 mb-8">
            物語を進めながら、実践的なハッキング技術とセキュリティ知識を学習
          </p>
          <div className="flex gap-4 justify-center">
            <Link
              href="/stories"
              className="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-8 rounded-lg transition-colors"
            >
              ストーリーを始める
            </Link>
            <Link
              href="/challenges"
              className="bg-gray-700 hover:bg-gray-600 text-white font-semibold py-3 px-8 rounded-lg transition-colors"
            >
              チャレンジ
            </Link>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-16">
          <div className="bg-white rounded-lg shadow-lg p-6">
            <h2 className="text-2xl font-semibold mb-3">ストーリーモード</h2>
            <p className="text-gray-600">
              5つのチャプターで構成された物語を進めながら、ハッキング技術を学びます。
            </p>
          </div>
          <div className="bg-white rounded-lg shadow-lg p-6">
            <h2 className="text-2xl font-semibold mb-3">ハッキングチャレンジ</h2>
            <p className="text-gray-600">
              実際のハッキング技術を実践できるインタラクティブなチャレンジです。
            </p>
          </div>
          <div className="bg-white rounded-lg shadow-lg p-6">
            <h2 className="text-2xl font-semibold mb-3">進捗管理</h2>
            <p className="text-gray-600">
              学習の進捗を追跡し、レベルアップしながらスキルを向上させます。
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
