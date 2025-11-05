import { createSelector, createFeatureSelector } from '@ngrx/store';
import { SidebarState } from './sidebar.reducer';

export const selectSidebarFeature =
  createFeatureSelector<SidebarState>('sidebar');

export const selectIsSidebarOpen = createSelector(
  selectSidebarFeature,
  (state) => state.isOpen
);
