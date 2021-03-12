export interface Message {
  id: number;
  senderId: number;
  senderUsername: string;
  senderPhotoUrl: string;
  recipiantId: number;
  recipiantUsername: string;
  recipiantPhotoUrl: string;
  content: string;
  dateRead?: Date;
  messageSent: Date;
}
