import { create } from 'zustand';
import { 
  signInWithEmailAndPassword, 
  createUserWithEmailAndPassword,
  signOut,
  onAuthStateChanged,
  updateProfile,
  User as FirebaseUser
} from 'firebase/auth';
import { auth } from '../firebase';
import { apiClient } from '../api';
import type { User } from '../../types/user';

interface AuthState {
  user: User | null;
  firebaseUser: FirebaseUser | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, username?: string) => Promise<void>;
  logout: () => Promise<void>;
  checkAuth: () => Promise<void>;
  initializeAuth: () => void;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  user: null,
  firebaseUser: null,
  isLoading: false,
  isAuthenticated: false,

  login: async (email: string, password: string) => {
    set({ isLoading: true });
    try {
      const userCredential = await signInWithEmailAndPassword(auth, email, password);
      const idToken = await userCredential.user.getIdToken();
      
      // バックエンドでトークンを検証し、ユーザー情報を取得
      const response = await apiClient.post<{ success: boolean; user: User }>(
        '/api/auth/verify',
        { idToken }
      );
      
      set({
        user: response.user,
        firebaseUser: userCredential.user,
        isAuthenticated: true,
        isLoading: false,
      });
    } catch (error) {
      set({ isLoading: false });
      throw error;
    }
  },

  register: async (email: string, password: string, username?: string) => {
    set({ isLoading: true });
    try {
      const userCredential = await createUserWithEmailAndPassword(auth, email, password);
      
      // ユーザー名を設定（Firebase AuthのdisplayName）
      if (username) {
        await updateProfile(userCredential.user, { displayName: username });
      }
      
      const idToken = await userCredential.user.getIdToken();
      
      // バックエンドでトークンを検証し、ユーザー情報を取得
      const response = await apiClient.post<{ success: boolean; user: User }>(
        '/api/auth/verify',
        { idToken }
      );
      
      set({
        user: response.user,
        firebaseUser: userCredential.user,
        isAuthenticated: true,
        isLoading: false,
      });
    } catch (error) {
      set({ isLoading: false });
      throw error;
    }
  },

  logout: async () => {
    try {
      await signOut(auth);
      apiClient.logout();
      set({
        user: null,
        firebaseUser: null,
        isAuthenticated: false,
      });
    } catch (error) {
      console.error('Logout error:', error);
    }
  },

  checkAuth: async () => {
    set({ isLoading: true });
    try {
      const user = await apiClient.getCurrentUser();
      set({
        user,
        isAuthenticated: true,
        isLoading: false,
      });
    } catch (error) {
      apiClient.logout();
      set({
        user: null,
        firebaseUser: null,
        isAuthenticated: false,
        isLoading: false,
      });
    }
  },

  initializeAuth: () => {
    if (typeof window === 'undefined') return;
    
    onAuthStateChanged(auth, async (firebaseUser) => {
      if (firebaseUser) {
        try {
          const idToken = await firebaseUser.getIdToken();
          const response = await apiClient.post<{ success: boolean; user: User }>(
            '/api/auth/verify',
            { idToken }
          );
          set({
            user: response.user,
            firebaseUser,
            isAuthenticated: true,
          });
        } catch (error) {
          console.error('Auth state change error:', error);
          set({
            user: null,
            firebaseUser: null,
            isAuthenticated: false,
          });
        }
      } else {
        set({
          user: null,
          firebaseUser: null,
          isAuthenticated: false,
        });
      }
    });
  },
}));

