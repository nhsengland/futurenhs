import { GenericPageContent } from '@appTypes/content';

interface Content extends GenericPageContent {}

export interface Props {
    content: Content;
    logOutUrl: string;
}