import {
  bootstrapApplication,
  BootstrapContext,
} from '@angular/platform-browser';
import { importProvidersFrom } from '@angular/core';
import { StoreModule } from '@ngrx/store';
import { AppComponent } from './app/app.component';
import { sidebarReducer } from './app/ai-sidebar/store/sidebar.reducer';
import { config } from './app/app.config.server';

const bootstrap = (context: BootstrapContext) =>
  bootstrapApplication(
    AppComponent,
    {
      ...config,
      providers: [
        ...(config.providers || []),
        importProvidersFrom(StoreModule.forRoot({ sidebar: sidebarReducer })),
      ],
    },
    context
  );

export default bootstrap;
