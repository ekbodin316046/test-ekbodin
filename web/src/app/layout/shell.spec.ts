import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { Shell } from './shell';

describe('Shell', () => {
  let fixture: ComponentFixture<Shell>;
  const realMatchMedia = window.matchMedia;

  // jsdom ships no matchMedia, so the viewport has to be declared per test.
  function stubViewport(narrow: boolean): void {
    window.matchMedia = ((query: string) =>
      ({
        matches: narrow && query.includes('900px'),
        media: query,
        onchange: null,
        addEventListener: () => undefined,
        removeEventListener: () => undefined,
        addListener: () => undefined,
        removeListener: () => undefined,
        dispatchEvent: () => false,
      }) as MediaQueryList) as typeof window.matchMedia;
  }

  async function render(): Promise<void> {
    fixture = TestBed.createComponent(Shell);
    fixture.detectChanges();
    await fixture.whenStable();
  }

  function overlay(): HTMLElement | null {
    return fixture.nativeElement.querySelector('.overlay');
  }

  function sidebarIsClosed(): boolean {
    return fixture.nativeElement.querySelector('.sidebar').classList.contains('closed');
  }

  async function clickMenuButton(): Promise<void> {
    fixture.nativeElement.querySelector('.menu-toggle').click();
    fixture.detectChanges();
    await fixture.whenStable();
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [Shell],
      providers: [provideRouter([])],
    });
  });

  afterEach(() => {
    window.matchMedia = realMatchMedia;
  });

  it('starts with the sidebar closed on a narrow screen', async () => {
    stubViewport(true);
    await render();

    expect(sidebarIsClosed()).toBe(true);
    expect(overlay()).toBeNull();
  });

  it('starts with the sidebar open on a wide screen', async () => {
    stubViewport(false);
    await render();

    expect(sidebarIsClosed()).toBe(false);
  });

  it('still opens the sidebar on demand on a narrow screen', async () => {
    stubViewport(true);
    await render();

    await clickMenuButton();

    expect(sidebarIsClosed()).toBe(false);
    expect(overlay()).not.toBeNull();
  });

  it('closes the sidebar when the overlay is tapped', async () => {
    stubViewport(true);
    await render();
    await clickMenuButton();

    overlay()!.click();
    fixture.detectChanges();
    await fixture.whenStable();

    expect(sidebarIsClosed()).toBe(true);
    expect(overlay()).toBeNull();
  });

  it('falls back to an open sidebar where matchMedia is unavailable', async () => {
    (window as { matchMedia?: typeof window.matchMedia }).matchMedia = undefined;
    await render();

    expect(sidebarIsClosed()).toBe(false);
  });
});
