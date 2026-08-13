import { Component, signal } from '@angular/core';
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
  // Opening by default on a phone would greet the user with the overlay
  // covering the page they came to read.
  protected readonly sidebarOpen = signal(!isNarrowViewport());

  protected toggleSidebar(): void {
    this.sidebarOpen.update((open) => !open);
  }
}

function isNarrowViewport(): boolean {
  return window.matchMedia?.(NARROW_VIEWPORT).matches === true;
}
