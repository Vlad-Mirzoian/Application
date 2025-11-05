import { createReducer, on } from '@ngrx/store';
import { openSidebar, closeSidebar, toggleSidebar } from './sidebar.actions';

export interface SidebarState {
  isOpen: boolean;
}

export const initialState: SidebarState = {
  isOpen: false,
};

export const sidebarReducer = createReducer(
  initialState,
  on(openSidebar, (state) => ({ ...state, isOpen: true })),
  on(closeSidebar, (state) => ({ ...state, isOpen: false })),
  on(toggleSidebar, (state) => ({ ...state, isOpen: !state.isOpen }))
);
