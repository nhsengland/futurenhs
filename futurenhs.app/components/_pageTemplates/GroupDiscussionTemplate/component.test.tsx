import React from 'react';
import * as nextRouter from 'next/router';
import { render, screen, cleanup } from '@testing-library/react';
import { actions as userActions } from '@constants/actions';
import { routes } from '@jestMocks/generic-props';
import formConfigs from '@formConfigs/index';

import { GroupDiscussionTemplate } from './index';
import { Props } from './interfaces';

describe('Group discussion template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/forum',
        query: {
            groupId: 'group',
            discussionId: '1'
        } 
    }));
    
    const props: Props = {
        id: 'mockId',
        routes: routes,
        tabId: 'forum',
        user: undefined,
        discussionId: 'mockDiscussionId',
        discussion: {
            createdBy: {
                id: 'mockUserId',
                text: {
                    userName: 'Mock username',
                }
            },
            text: {
                title: 'Mock discussion title',
                body: 'Mock discussion body text'
            },
            created: '01/01/2022',
            responseCount: 99,
            viewCount: 250
        },
        discussionCommentsList: [
            {
                commentId: 'Mock comment id',
                text: {
                    body: 'Mock comment text'
                },
                replies: [
                    {
                        commentId: 'Mock reply id',
                        text: {
                            body: 'Mock reply text'
                        }
                    }
                ]
            }
        ],
        contentText: {
            lastCommentLabel: 'Last comment by',
            totalRecordsLabel: 'comments',
            viewCountLabel: 'views',
            moreRepliesLabel: 'Show more replies',
            secondaryHeading: 'Join in the conversation'
        },
        entityText: {
            title: 'Mock title',
            metaDescription: 'Mock meta description',
            mainHeading: 'Mock main heading',
            strapLine: 'Mock strapline'
        },
        image: null,
        actions: [],
        forms: formConfigs,
        pagination: {
            totalRecords: 99
        }
    }

    it('renders correctly', () => {
        
        render(<GroupDiscussionTemplate {...props}/>);
        
        expect(screen.getAllByText('Mock discussion title').length).toBe(1);

    });

    it('conditionally renders discussion body', () => {

        render(<GroupDiscussionTemplate {...props}/>);

        expect(screen.getAllByText('Mock discussion body text').length).toBe(1);

        cleanup();

        const propsCopy = JSON.parse(JSON.stringify(props));

        delete propsCopy.discussion.text.body

        render(<GroupDiscussionTemplate {...propsCopy}/>);

        expect(screen.queryByText('Mock discussion body text')).toBeNull();

    });

    it('conditionally renders last comment by section if a comment has been made', () => {
        
        render(<GroupDiscussionTemplate {...props}/>);

        expect(screen.queryByText('Last comment by')).toBeNull();

        cleanup();

        const propsCopy = JSON.parse(JSON.stringify(props));

        propsCopy.discussion.responseCount = 5;
        propsCopy.discussion.modifiedBy = {
            id: 'mockId',
            text: {
                userName: 'Mock last comment username'
            }
        }

        render(<GroupDiscussionTemplate {...propsCopy}/>);

        expect(screen.getAllByText('Last comment by').length).toBe(1);
        expect(screen.getAllByText('Mock last comment username').length).toBe(1);


    });

    it('conditionally renders comment count if there are any comments', () => {

        render(<GroupDiscussionTemplate {...props}/>);

        expect(screen.getAllByText('Comments: 99').length).toBeGreaterThan(0);

        cleanup();

        const propsCopy: Props = Object.assign({}, props);

        props.discussion.responseCount = 0;

        render(<GroupDiscussionTemplate {...propsCopy}/>);

        expect(screen.queryByText('Comments: 99')).toBeNull();
        
        
    });

    it('conditionally renders discussion comments', () => {

        render(<GroupDiscussionTemplate {...props}/>);

        expect(screen.getAllByText('Mock comment text').length).toBe(1);

        cleanup();

        const propsCopy: Props = Object.assign({}, props, {discussionCommentsList: null});

        render(<GroupDiscussionTemplate {...propsCopy}/>);

        expect(screen.queryByText('Mock comment text')).toBeNull();
        
    });

    it('conditionally renders comment replies', () => {
        
        render(<GroupDiscussionTemplate {...props}/>);

        expect(screen.getAllByText('Mock reply text').length).toBe(1);

        cleanup();

        const propsCopy = JSON.parse(JSON.stringify(props));
        propsCopy.discussionCommentsList[0].replies = null;

        render(<GroupDiscussionTemplate {...propsCopy}/>);

        expect(screen.queryByText('Mock reply text')).toBeNull();

    });

    it('conditionally render accordion if there is more than one comment', () => {
        
        const propsCopy = JSON.parse(JSON.stringify(props));
        propsCopy.discussionCommentsList[0].replies[1] =   {
            commentId: 'Mock reply id 2',
            text: {
                body: 'Mock reply text 2'
            }
        }

        render(<GroupDiscussionTemplate {...propsCopy}/>);

        expect(screen.getAllByText('Show more replies').length).toBe(1);

        cleanup();

        render(<GroupDiscussionTemplate {...props}/>)
        
        expect(screen.queryByText('Show more replies')).toBeNull();

    });

    it('conditionally renders comment and reply forms', () => {

        const propsCopy: Props = Object.assign({}, props, {
            actions: [
                userActions.GROUPS_COMMENTS_ADD
            ],
            user: {
                id: 'testUserId',
                text: {
                    userName: 'Test username'
                }
            }
        })

        render(<GroupDiscussionTemplate {...propsCopy}/>);

        expect(screen.getAllByText('Join in the conversation').length).toBe(1);

        cleanup();

        render(<GroupDiscussionTemplate {...props}/>);

        expect(screen.queryByText('Join in the conversation')).toBeNull();

        
    });
});