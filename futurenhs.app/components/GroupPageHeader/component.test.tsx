import { render, screen, cleanup } from '@testing-library/react';

import { actions as actionConstants } from '@constants/actions';
import { routes } from '@jestMocks/generic-props';
import { GroupPageHeader } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    id: 'mockId',
    text: {
        mainHeading: 'mockHeading',
        description: 'mockDescription',
        navMenuTitle: 'mockNavMenuTitleText'   
    },
    routes: routes,
    navMenuList: []
};

describe('GroupPageHeader', () => {

    it('renders heading Html', () => {

        const props = Object.assign({}, testProps);

        render(<GroupPageHeader {...props} />);

        expect(screen.getByText('mockHeading'));

    });

    it('conditionally renders group join link', () => {

        render(<GroupPageHeader {...testProps} />);

        expect(screen.queryByText('Join Group')).toBeNull();

        cleanup();

        const propsCopy: Props = Object.assign({}, testProps, {
            actions: [
                actionConstants.GROUPS_JOIN
            ]
        })

        render(<GroupPageHeader {...propsCopy} />);

        expect(screen.getAllByText('Join Group').length).toBe(1);


    });

    it('conditionally renders actions in actions menu', () => {

        render(<GroupPageHeader {...testProps} />);

        expect(screen.queryByText('Edit group information')).toBeNull();
        expect(screen.queryByText('Leave group')).toBeNull();
        expect(screen.queryByText('Page manager')).toBeNull();

        cleanup();

        const propsCopy: Props = Object.assign({}, testProps, {
            actions: [
                actionConstants.GROUPS_LEAVE,
                actionConstants.GROUPS_EDIT,
                actionConstants.SITE_ADMIN_GROUPS_EDIT
            ]
        })  

        render(<GroupPageHeader {...propsCopy} />);

        expect(screen.getAllByText('Edit group information').length).toBe(1);
        expect(screen.getAllByText('Leave group').length).toBe(1);
        expect(screen.getAllByText('Page manager').length).toBe(1);

    });
    
});
