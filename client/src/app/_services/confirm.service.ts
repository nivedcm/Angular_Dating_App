import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { title } from 'node:process';
import { Observable } from 'rxjs';
import { ConfirmBoxComponent } from '../modals/confirm-box/confirm-box.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModelRef: BsModalRef;

  constructor(private modalService: BsModalService) { }

  confirm(title='Confirm',
  message='Are you sure you want to do this?',
  btnOkText = 'Ok',
  btnCancelText ='Cancel'): Observable<boolean> {
    const config = {
      initialState: {
        title,
        message,
        btnOkText,
        btnCancelText
      }
    }
    this.bsModelRef = this.modalService.show(ConfirmBoxComponent, config);
    return new Observable<boolean>(this.GetResult());
  }

  private GetResult() {
    return (observer) => {
      const subscription = this.bsModelRef.onHidden.subscribe(() => {
        observer.next(this.bsModelRef.content.result);
        observer.complete();
    });

    return{
      unsubscribe(){
        subscription.unsubscribe();
      }
    }
  }
}
}
