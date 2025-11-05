import { bootstrapApplication } from '@angular/platform-browser';
import { importProvidersFrom } from '@angular/core';
import { StoreModule } from '@ngrx/store';
import { AppComponent } from './app/app.component';
import { sidebarReducer } from './app/ai-sidebar/store/sidebar.reducer';
import { appConfig } from './app/app.config';

bootstrapApplication(AppComponent, {
  ...appConfig,
  providers: [
    ...(appConfig.providers || []),
    importProvidersFrom(StoreModule.forRoot({ sidebar: sidebarReducer })),
  ],
}).catch((err) => console.error(err));
