import { actions } from '@constants/actions';
import { Image } from './image';

import { layoutIds } from '@constants/routes';
import { Pagination } from '@appTypes/pagination';
import { FormConfig } from '@appTypes/form';
import { Routes } from '@appTypes/routing';
import { Service } from '@appTypes/service';
import { ContentBlock } from '@components/ContentBlock';
import { GenericPageTextContent, GroupsPageTextContent } from '@appTypes/content';
import { User } from '@appTypes/user';

export interface Page {
    id: string;
    routes: Routes;
    etag?: string | Record<string, string>;
    themeId?: string;
    layoutId?: layoutIds;
    actions?: Array<actions>;
    csrfToken?: string;
    forms?: Record<string, FormConfig>;
    services?: Record<string, Service>;
    pagination?: Pagination;
    errors?: Array<Record<string>>;
    contentText?: GenericPageTextContent;
    contentPageId?: string;
    contentTemplateId?: string;
    contentTemplate?: Array<ContentBlock>;
    contentBlocks?: Array<ContentBlock>;
    user?: User;
    className?: string;
    pageTitle?: string;
}

export interface GroupPage extends Page {
    groupId?: string;
    tabId: 'index' | 'forum' | 'files' | 'members' | 'about';
    imageId?: string;
    image: Image;
    contentText: GroupsPageTextContent;
    entityText: any;
}
