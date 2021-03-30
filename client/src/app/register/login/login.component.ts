import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  @Output() cancelLogin = new EventEmitter();
  loginForm: FormGroup;
  model: any = {};
  validationErrors: string[] = [];

  constructor(
    private toastr: ToastrService,
    private accountService: AccountService,
    private formbuilder: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.loginForm = this.formbuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    })
  }

  login(){
    this.accountService.login(this.model).subscribe(response => {
        this.router.navigateByUrl('/members');
    },error => {
        this.toastr.error(error.error);
    })
  }

  cancel() {
    this.cancelLogin.emit(false);
  }

  forgotPassword() {
    this.router.navigateByUrl('../forgot-password/');
  }
}
