import React from 'react';
import * as nextRouter from 'next/router';
import { render, screen, cleanup } from '@testing-library/react';

import { createFolderForm } from '@formConfigs/create-folder';
import { GroupCreateFolderTemplate } from './index';
import { Props } from './interfaces';

describe('Group folders template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/files',
        query: {
            groupId: 'group'
        } 
    }));

    const props: Props = {
        id: 'mockId',
        tabId: 'files',
        folderId: 'mockId',
        folder: {
            id: 'mockId',
            type: 'folder',
            text: {
                name: 'Mock folder name'
            }
        },
        user: undefined,
        actions: [],
        forms: {
            'create-folder': createFolderForm
        },
        contentText: null,
        entityText: null,
        image: null
    };

    it('renders correctly', () => {

        render(<GroupCreateFolderTemplate {...props} />);

        expect(screen.getAllByText('Save and continue').length).toEqual(1);

    });

    it('conditionally renders folder name when passed folder prop', () => {
        
        render(<GroupCreateFolderTemplate {...props} />)

        expect(screen.getAllByText('Mock folder name').length).toEqual(1);

        cleanup();

        const propsCopy = Object.assign({}, props)
        delete propsCopy.folder;

        render(<GroupCreateFolderTemplate {...propsCopy} />)

        expect(screen.queryByText('Mock folder name')).toBeNull();

    });


});
