import { Image } from './image';

export interface Member {
    id?: string;
    role?: string;
    firstName?: string;
    lastName?: string;
    fullName?: string;
    pronouns?: string;
    email?: string;
    requestDate?: string;
    joinDate?: string;
    lastLogInDate?: string;
    image?: Image;
    imageId?: string;
    roleId?: string;
}