import { sidebarReducer, SidebarState } from './sidebar.reducer';
import { ActionReducerMap } from '@ngrx/store';

export interface AppState {
  sidebar: SidebarState;
}

export const appReducers: ActionReducerMap<AppState> = {
  sidebar: sidebarReducer,
};
