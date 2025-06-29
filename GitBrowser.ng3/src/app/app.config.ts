import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withFetch } from '@angular/common/http'; // Added

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideHttpClient(withFetch()) // Added
    // Removed provideBrowserGlobalErrorListeners as it's not standard for minimal new apps and might not be needed.
    // If errors occur, it can be added back.
  ]
};
