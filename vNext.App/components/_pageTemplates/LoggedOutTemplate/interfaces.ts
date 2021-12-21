import { Page } from '@appTypes/page';
import { GenericPageContent } from '@appTypes/content';

interface Content extends GenericPageContent {}

export interface Props extends Page {
    content: Content;
    logOutUrl: string;
}