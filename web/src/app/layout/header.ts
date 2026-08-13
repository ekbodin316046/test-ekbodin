import { Component, output } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class AppHeader {
  readonly menuToggled = output<void>();
}
