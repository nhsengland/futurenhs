import React, { useState } from 'react'
import { DataGrid } from './index'
import { capitalise } from '@helpers/formatters/capitalise'
import { dateTime } from '@helpers/formatters/dateTime'
import { Link } from '@components/Link'
import { ActionLink } from '@components/ActionLink'
import classNames from 'classnames'

export default {
    title: 'Data Grid',
    component: DataGrid,
    argTypes: {
        id: {
            control: { type: '' },
        },
        columnList: {
            control: { type: '' },
        },
        rowList: {
            control: { type: '' },
        },
        className: {
            control: { type: '' },
        },
    }  
}

const users = [
    {
        id: 1,
        fullName: 'Stephen Stephenson',
        role: 'Admin',
        joinDate: '2022-04-11T14:41:52Z',
        lastLogInDate: '2022-05-04T08:19:24Z'
    },
    {
        id: 2,
        fullName: 'John Johnson',
        role: 'Standard Member',
        joinDate: '2022-01-20T14:41:52Z',
        lastLogInDate: '2022-03-07T08:19:24Z'
    },
    {
        id: 2,
        fullName: 'Andrew Anderson',
        role: 'Standard Member',
        joinDate: '2022-04-12T14:41:52Z',
        lastLogInDate: '2022-05-01T08:19:24Z'
    }
]

const columnList = [
    {
        children: 'Name',
        className: '',
    },
    {
        children: 'Role',
        className: '',
    },
    {
        children: 'Date joined',
        className: '',
    },
    {
        children: 'Last logged in',
        className: '',
    },
    {
        children: `Actions`,
        className: 'tablet:u-text-right',
    },
]

const rowList = users.map(
    ({ id, fullName, role, joinDate, lastLogInDate }) => {
        const generatedCellClasses = {
            name: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/4 o-truncated-text-lines-1']:
                    true,
            }),
            role: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/4']: true,
            }),
            joinDate: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/6']: true,
            }),
            lastLoginDate: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/6']: true,
            }),
        }

        const generatedHeaderCellClasses = {
            name: classNames({
                ['u-text-bold']: true,
            }),
            role: classNames({
                ['u-text-bold']: true,
            }),
            joinDate: classNames({
                ['u-text-bold']: true,
            }),
            lastLoginDate: classNames({
                ['u-text-bold']: true,
            }),
        }

        const rows = [
            {
                children: (
                    <Link href={`/users/${id}`}>{fullName}</Link>
                ),
                className: generatedCellClasses.name,
                headerClassName: generatedHeaderCellClasses.name,
            },
            {
                children: `${capitalise({ value: role })}`,
                className: generatedCellClasses.role,
                headerClassName: generatedHeaderCellClasses.role,
                shouldRenderCellHeader: true,
            },
            {
                children: `${dateTime({ value: joinDate })}`,
                className: generatedCellClasses.joinDate,
                headerClassName:
                    generatedHeaderCellClasses.joinDate,
                shouldRenderCellHeader: true,
            },
            {
                children: `${dateTime({ value: lastLogInDate })}`,
                className: generatedCellClasses.lastLoginDate,
                headerClassName:
                    generatedHeaderCellClasses.lastLoginDate,
                shouldRenderCellHeader: true,
            },
            {
                children: 
                    <ActionLink 
                        href={`/users/${id}?edit=true`}
                        text={{
                            body: 'Edit',
                            ariaLabel: `Edit user ${fullName || role}`
                        }}
                        iconName="icon-edit" />,
                className:
                    'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                headerClassName: 'u-hidden',
            },
        ]

        return rows
    }
)


const Template = (args) => <DataGrid columnList={columnList} rowList={rowList} {...args}/>

export const Basic = Template.bind({})
Basic.args = {
    id: "123",
    shouldRenderCaption: true,
    text: {
        caption: 'Table Caption'
    }
}