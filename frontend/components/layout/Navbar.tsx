'use client';

import Link from 'next/link';
import { useAuthStore } from '../../lib/stores/authStore';
import { useRouter } from 'next/navigation';

export default function Navbar() {
  const { user, isAuthenticated, logout } = useAuthStore();
  const router = useRouter();

  const handleLogout = () => {
    logout();
    router.push('/');
  };

  return (
    <nav className="bg-gray-800 text-white">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center">
            <Link href="/" className="text-xl font-bold">
              HackStory
            </Link>
            {isAuthenticated && (
              <div className="ml-10 flex items-baseline space-x-4">
                <Link
                  href="/stories"
                  className="px-3 py-2 rounded-md text-sm font-medium hover:bg-gray-700"
                >
                  ストーリー
                </Link>
              </div>
            )}
          </div>
          <div className="flex items-center space-x-4">
            {isAuthenticated ? (
              <>
                <span className="text-sm">
                  {user?.username || user?.email}
                </span>
                <button
                  onClick={handleLogout}
                  className="px-4 py-2 rounded-md text-sm font-medium bg-gray-700 hover:bg-gray-600"
                >
                  ログアウト
                </button>
              </>
            ) : (
              <>
                <Link
                  href="/auth/login"
                  className="px-4 py-2 rounded-md text-sm font-medium hover:bg-gray-700"
                >
                  ログイン
                </Link>
                <Link
                  href="/auth/register"
                  className="px-4 py-2 rounded-md text-sm font-medium bg-blue-600 hover:bg-blue-700"
                >
                  登録
                </Link>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}

