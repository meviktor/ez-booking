import { Component, OnDestroy } from '@angular/core';
import { AnimationTriggerMetadata, animate, state, style, transition, trigger } from '@angular/animations';

import { Subscription, interval, map } from 'rxjs';

enum AnimationState {
  Start = 'start',
  End = 'end'
}

const ANIMATION_DURATION: number = 10000;
const fade: AnimationTriggerMetadata = trigger('fade', [
  state(AnimationState.End, style({ opacity: 0.66 })),
  state(AnimationState.Start, style({ opacity: 1 })),
  transition(`${AnimationState.Start} => ${AnimationState.End}`, [animate(ANIMATION_DURATION)])
]);

@Component({
  selector: 'app-login-slideshow',
  templateUrl: './loginslideshow.component.html',
  animations: [fade]
})
export class LoginSlideShowComponent implements OnDestroy {
  private imgSrcChangeSubscription: Subscription;

  public animationState: AnimationState = AnimationState.Start;
  public imgSrcArray: string[] = [
    '../../../../assets/img/loginslideshow/entrepreneurs-discussing-work-results-meeting.jpg',
    '../../../../assets/img/loginslideshow/skyscrapers-from-low-angle-view.jpg',
    '../../../../assets/img/loginslideshow/view-modern-office.jpg'
  ];
  public imgSrc: string;

  constructor() {
    this.imgSrc = this.imgSrcArray?.[0] ?? "";
    this.imgSrcChangeSubscription = interval(ANIMATION_DURATION)
      .pipe(map((x: number) => x + 1))
      .subscribe((x: number) => this.imgSrc = this.imgSrcArray[x % this.imgSrcArray.length]);
  }

  ngOnDestroy(): void {
    this.imgSrcChangeSubscription.unsubscribe();
  }

  public onFadeFinished($event: any): void {
    this.animationState = this.animationState == AnimationState.Start ? AnimationState.End : AnimationState.Start;
  }
}
