import { Pagination } from '@appTypes/pagination';
import { GenericPageContent, GroupsPageContent } from '@appTypes/content';
import { User } from '@appTypes/user';
import { Image } from './image';

export interface Page {
    id: string;
    csrfToken?: string;
    pagination?: Pagination;
    errors?: Record<string>;
    content?: GenericPageContent;
    user?: User;
}

export interface GroupPage extends Page {
    image: Image;
    content: GroupsPageContent;
}
