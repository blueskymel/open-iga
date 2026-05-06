
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { providers } from './app/app.providers';

bootstrapApplication(AppComponent, { providers })
  .catch((err) => console.error(err));
