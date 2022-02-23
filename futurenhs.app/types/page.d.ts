import { actions } from '@constants/actions';
import { Image } from './image';

import { Pagination } from '@appTypes/pagination';
import { FormConfig } from '@appTypes/form';
import { Service } from '@appTypes/service';
import { GenericPageTextContent, GroupsPageTextContent } from '@appTypes/content';
import { User } from '@appTypes/user';

export interface Page {
    id: string;
    actions?: Array<actions>;
    csrfToken?: string;
    forms?: Record<string, FormConfig>;
    services?: Record<string, Service>;
    pagination?: Pagination;
    errors?: Array<Record<string>>;
    contentText?: GenericPageTextContent;
    user?: User;
    themeId?: string;
}

export interface GroupPage extends Page {
    groupId?: string;
    image: Image;
    contentText: GroupsPageTextContent;
    entityText: any;
}
