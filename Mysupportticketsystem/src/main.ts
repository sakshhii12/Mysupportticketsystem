import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app'; // This is your root component

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
