export interface CmsContentBlock {
    item: {
        id: string;
        contentType: string;
        name?: string;
    };
    content: Record<any, any>;
}
