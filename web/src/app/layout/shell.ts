import { Component, DestroyRef, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { AppHeader } from './header';
import { Sidebar } from './sidebar';

// Matches the breakpoint in shell.css where the sidebar starts floating over
// the content behind a dark overlay.
const NARROW_VIEWPORT = '(max-width: 900px)';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, AppHeader, Sidebar],
  templateUrl: './shell.html',
  styleUrl: './shell.css',
})
export class Shell {
  private readonly viewport = window.matchMedia?.(NARROW_VIEWPORT);

  // Opening by default on a phone would greet the user with the overlay
  // covering the page they came to read.
  protected readonly sidebarOpen = signal(!this.viewport?.matches);

  constructor() {
    if (!this.viewport) {
      return;
    }

    // Crossing the breakpoint changes what the sidebar means, so the answer
    // from load time stops applying.
    const onViewportChange = (event: MediaQueryListEvent) => this.sidebarOpen.set(!event.matches);

    this.viewport.addEventListener('change', onViewportChange);
    inject(DestroyRef).onDestroy(() => this.viewport?.removeEventListener('change', onViewportChange));
  }

  protected toggleSidebar(): void {
    this.sidebarOpen.update((open) => !open);
  }
}
